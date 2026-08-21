export async function registerServiceWorker() {
    await navigator.serviceWorker.register(
        "_content/Avae.Blazor.Notifications/service-worker.js",
        { scope: "/" }
    );
}

export function requestPermission() {
    return Notification.requestPermission();
}

export function isSupported() {
    return 'Notification' in window;
}

export async function closeAllNotifications() {
    const registration = await navigator.serviceWorker.ready;
    const notifications = await registration.getNotifications();
    notifications.forEach(n => n.close());
    return notifications.length;
}

export async function getNotifications() {
    const registration = await navigator.serviceWorker.ready;
    const notifications = await registration.getNotifications();
    return notifications.map(n => ({
        title: n.title,
        body: n.body,
        tag: n.tag,
        icon: n.icon,
        badge: n.badge,
        image: n.image,
        lang: n.lang,
        dir: n.dir,
        requireInteraction: n.requireInteraction,
        silent: n.silent,
        timestamp: n.timestamp,          // number (ms since epoch)
        data: n.data,                    // any JSON-serializable value you previously set
        // actions are also available in modern browsers:
        actions: n.actions ? n.actions.map(a => ({
            action: a.action,
            title: a.title,
            icon: a.icon,
            type: a.type
        })) : []
    }));
}

export async function create(title, options) {
    const registration = await navigator.serviceWorker.ready;
    await registration.showNotification(title, options);
}

export const registrations = {
    dotNetHelper: null,
    registerDotnet: function (helper)
    {
        registrations.dotNetHelper = helper;
        navigator.serviceWorker.addEventListener('message', function (event) {            
            if (event.data && event.data.type === 'HandleNotificationClose') {
                registrations.handleClose(event);
            }
            if (event.data && event.data.type === 'HandleNotificationClick') {
                registrations.handleNotificationClick(event);
            }
            if (event.data && event.data.type === 'HandleNotificationReply') {
                registrations.handleNotificationReply(event);
            }
        });
    },
    handleNotificationClick: function (event) {
        registrations.dotNetHelper.invokeMethodAsync('HandleNotificationClick', event.data);
    },
    handleNotificationReply: function (event) {
        registrations.dotNetHelper.invokeMethodAsync('HandleNotificationReply', event.data, event.data.reply);
    },
    handleClose: function (event) {
        registrations.dotNetHelper.invokeMethodAsync('HandleNotificationClose', event.data);
    }
};