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

export async function getNotifications() {
    const registration = await navigator.serviceWorker.ready;
    return await registration.getNotifications();
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