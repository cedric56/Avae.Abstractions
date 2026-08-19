export async function create(title, options) {
    const reg = await navigator.serviceWorker.ready;
    return reg.showNotification(title, options);
}