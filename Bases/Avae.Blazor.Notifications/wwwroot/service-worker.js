self.addEventListener('install', function (event) {
    console.log('Service Worker installed');
    event.waitUntil(self.skipWaiting());
});

self.addEventListener('activate', function (event) {
    console.log('Service Worker activated');
    event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', function (event) {
    // Basic fetch handling
    event.respondWith(fetch(event.request));
});

// Handle notification clicks
self.addEventListener('notificationclick', function (event) {
    console.log('Notification clicked:', event.notification);
    event.notification.close();

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true })
            .then(function (clientList) {
                // Open or focus an existing window
                for (var i = 0; i < clientList.length; i++) {
                    var client = clientList[i];
                    if (client.url === '/' && 'focus' in client) {
                        return client.focus();
                    }
                }
                if (clients.openWindow) {
                    return clients.openWindow('/');
                }
            })
    );
});