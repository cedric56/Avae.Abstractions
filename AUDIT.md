# Avae.Abstractions — Audit technique

> Audit du dépôt `cedric56/Avae.Abstractions` sur `master`.
>
> Cette PR documente les constats et crée une issue GitHub par sujet. Elle ne corrige pas encore le code de production.

## Synthèse

| Priorité | Sujet | Issue |
|---|---|---|
| 🔴 High | Configuration NuGet ignorée | #1 |
| 🔴 High | Cast concret derrière `IDialogService` | #2 |
| 🔴 High | `ModalService` statique | #3 |
| 🔴 High | État natif statique du `DialogService` | #4 |
| 🟠 Medium | `ViewFor.Context` non déterministe | #5 |
| 🟠 Medium | Recréation du `ControlTemplate` | #6 |
| 🟠 Medium | Host natif de notifications non lifecycle-aware | #7 |
| 🟠 Medium | Résolution de fenêtre implicite | #8 |
| 🟡 Low | `TaskCompletionSource` sans continuations asynchrones | #9 |
| 🟠 Medium | APIs MAUI non supportées hors Windows | #10 |

## 1. Configuration NuGet ignorée

Le dépôt contient/documente `nuget..config` avec deux points.

NuGet ne reconnaît pas ce nom. La configuration de feed prévue peut donc être ignorée silencieusement lors d'un restore propre.

### Recommandation

Renommer en `nuget.config`, vérifier le restore depuis un clone propre et aligner le README.

Voir [#1](https://github.com/cedric56/Avae.Abstractions/issues/1).

## 2. Cast concret de IDialogService

`ContentDialogService` et `TaskDialogService` reçoivent `IDialogService`, puis le castent en `DialogService`.

Cela rompt l'abstraction DI : une autre implémentation de l'interface provoquera un `InvalidCastException`.

### Recommandation

Déplacer l'opération nécessaire dans l'interface appropriée ou introduire une interface dédiée.

Voir [#2](https://github.com/cedric56/Avae.Abstractions/issues/2).

## 3. ModalService statique

`ModalService` est une classe statique et s'appuie sur l'état global du système de dialogs.

Cela réduit la testabilité et rend le choix de la fenêtre ambigu en multi-window.

### Recommandation

Introduire `IModalService`, l'enregistrer dans DI et rendre le contexte UI explicite.

Voir [#3](https://github.com/cedric56/Avae.Abstractions/issues/3).

## 4. État natif statique du DialogService

Le dialog Android utilise un état mutable statique partagé avec `ModalService`.

Une Activity peut être recréée alors que la référence native statique existe encore.

### Recommandation

Lier l'état natif à l'instance/fenêtre/Activity qui le possède et nettoyer les références lors de son cycle de vie.

Voir [#4](https://github.com/cedric56/Avae.Abstractions/issues/4).

## 5. ViewFor.Context

Le setter de `Context` appelle `Dispatcher.Dispatch` hors thread UI. Le setter peut donc retourner avant que le `BindingContext` soit réellement modifié.

### Risque

Le code de navigation peut continuer avec une vue dont le contexte n'est pas encore à jour.

### Recommandation

Garantir l'exécution UI à la frontière de navigation, ou rendre explicitement l'opération asynchrone.

Voir [#5](https://github.com/cedric56/Avae.Abstractions/issues/5).

## 6. AvaeEntry et ControlTemplate

`AvaeEntry.OnBindingContextChanged` recrée le `ControlTemplate` à chaque changement de BindingContext.

Le template étant structurel, cette reconstruction provoque du churn inutile du visual tree.

### Recommandation

Créer le template une seule fois et conserver les bindings dynamiques.

Voir [#6](https://github.com/cedric56/Avae.Abstractions/issues/6).

## 7. NotificationService et état natif

Le service est enregistré en singleton et les implémentations natives conservent du state lié à l'UI native.

Un host Android peut devenir invalide après recréation de l'Activity.

### Recommandation

Associer le host à son Activity/fenêtre et le recréer lorsque son propriétaire change.

Voir [#7](https://github.com/cedric56/Avae.Abstractions/issues/7).

## 8. DialogService.Current et multi-window

La résolution actuelle choisit une fenêtre activée, puis la première fenêtre disponible, avec fallback vers Shell.

Cette stratégie n'identifie pas nécessairement la fenêtre qui a demandé le dialog.

### Recommandation

Passer la fenêtre cible explicitement ou utiliser un provider de contexte window-aware.

Voir [#8](https://github.com/cedric56/Avae.Abstractions/issues/8).

## 9. TaskCompletionSource

Les callbacks natifs complètent des `TaskCompletionSource<T>` sans `RunContinuationsAsynchronously`.

Cela permet l'exécution inline de continuations depuis un callback UI/native et augmente les risques de réentrance.

### Recommandation

Utiliser :

```csharp
new TaskCompletionSource<T?>(
    TaskCreationOptions.RunContinuationsAsynchronously);
```

Voir [#9](https://github.com/cedric56/Avae.Abstractions/issues/9).

## 10. Support plateforme incomplet

Le README indique que `NotificationService.Show` et une partie de `ModalService` sont actuellement Windows-only et lèvent `NotImplementedException` sur Android/iOS/MacCatalyst.

Le problème n'est pas nécessairement l'absence d'implémentation elle-même, mais le fait que les interfaces sont enregistrées comme disponibles alors que leur capacité réelle dépend de la plateforme.

### Recommandation

Soit implémenter les plateformes manquantes, soit exposer explicitement la capacité/support et documenter le contrat au niveau de l'API.

Voir [#10](https://github.com/cedric56/Avae.Abstractions/issues/10).

## Ordre de traitement proposé

1. #1 — corriger le restore NuGet.
2. #2 — rétablir une vraie abstraction de dialog.
3. #3/#4/#8 — supprimer la dépendance à l'état global et clarifier la fenêtre cible.
4. #5 — sécuriser la frontière UI/navigation.
5. #7/#10 — traiter le cycle de vie et les capacités par plateforme.
6. #6/#9 — robustesse et optimisation complémentaires.

## Périmètre

Cette PR est volontairement documentaire : elle ne modifie pas encore les implémentations. Chaque correction peut ainsi être traitée et testée indépendamment via son issue.

