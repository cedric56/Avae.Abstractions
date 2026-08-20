export function requestPermission() {
    return Notification.requestPermission();
}

export function isSupported() {
    return 'Notification' in window;
}

export async function create(title, options) {
    // Convert options for service worker
    const notificationOptions = {
        body: options.body || '',
        // icon: options.icon || '/icon-192x192.png',
        // badge: '/badge-72x72.png',
        // vibrate: [200, 100, 200],
        action: options.action || {},
        id: options.id || {},
        data: options.data || {},
        actions: options.actions || [],
        tag: options.tag || `notification-${Date.now()}`
    };
    // Show notification through service worker
    const registration = await navigator.serviceWorker.ready;
    await registration.showNotification(title, notificationOptions);
}

export const registrations = {
    dotNetHelper: null,
    registerDotnet: function (helper)
    {
        registrations.dotNetHelper = helper;
        navigator.serviceWorker.addEventListener('message', function (event) {
            console.log('Page received message from service worker:', event.data);

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

    // Handle notification input reply
    handleNotificationReply: function (event) {
        console.log('HandleNotificationReply:', event);
        registrations.dotNetHelper.invokeMethodAsync('HandleNotificationReply', event.data, event.action, event.replyText);
    },
    handleClose: function (event) {
        registrations.dotNetHelper.invokeMethodAsync('HandleNotificationClose', event.data);
    }
};