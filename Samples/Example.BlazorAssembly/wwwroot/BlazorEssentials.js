let connectivityHandler = null;

function readConnectivity() {
    const c = navigator.connection;
    return {
        online: navigator.onLine,
        type: c?.type ?? "unknown",       // Chromium only: wifi, cellular, ethernet, bluetooth...
        saveData: c?.saveData ?? false
    };
}

export function connectivityGetSnapshot() {
    return readConnectivity();
}

export function connectivitySubscribe(dotNetRef) {
    connectivityUnsubscribe();
    connectivityHandler = () =>
        dotNetRef.invokeMethodAsync("OnBrowserChanged", readConnectivity());
    window.addEventListener("online", connectivityHandler);
    window.addEventListener("offline", connectivityHandler);
    navigator.connection?.addEventListener("change", connectivityHandler);
}

export function connectivityUnsubscribe() {
    if (!connectivityHandler) return;
    window.removeEventListener("online", connectivityHandler);
    window.removeEventListener("offline", connectivityHandler);
    navigator.connection?.removeEventListener("change", connectivityHandler);
    connectivityHandler = null;
}

export function appInfoGet() {
    return {
        title: document.title,
        hostname: location.hostname,
        rtl: document.documentElement.dir === "rtl"
            || getComputedStyle(document.documentElement).direction === "rtl",
        prefersDark: window.matchMedia("(prefers-color-scheme: dark)").matches
    };
}

let appInfoMql = null;
let appInfoHandler = null;

export function appInfoSubscribeTheme(dotNetRef) {
    appInfoUnsubscribeTheme();
    appInfoMql = window.matchMedia("(prefers-color-scheme: dark)");
    appInfoHandler = e => dotNetRef.invokeMethodAsync("OnThemeChanged", e.matches);
    appInfoMql.addEventListener("change", appInfoHandler);
}

export function appInfoUnsubscribeTheme() {
    if (appInfoMql && appInfoHandler) {
        appInfoMql.removeEventListener("change", appInfoHandler);
    }
    appInfoMql = null;
    appInfoHandler = null;
}

export function pickFiles(accept, multiple) {
    return new Promise(resolve => {
        const input = document.createElement("input");
        input.type = "file";
        input.multiple = !!multiple;
        if (accept) input.accept = accept;
        input.style.display = "none";

        input.addEventListener("change", async () => {
            const files = Array.from(input.files ?? []);
            const results = await Promise.all(files.map(f => new Promise((res, rej) => {
                const reader = new FileReader();
                reader.onload = () => {
                    const base64 = reader.result.substring(reader.result.indexOf(",") + 1);
                    res({ name: f.name, type: f.type, dataBase64: base64 });
                };
                reader.onerror = () => rej(reader.error);
                reader.readAsDataURL(f);
            })));
            document.body.removeChild(input);
            resolve(results);
        });

        // Some browsers require the input to be in the DOM to show the dialog reliably.
        document.body.appendChild(input);
        input.click();
    });
}