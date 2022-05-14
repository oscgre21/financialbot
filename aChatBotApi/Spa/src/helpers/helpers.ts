
export function getManUrl() {
    return 'http://localhost:27760/';//document.getElementsByTagName('base')[0].href;
}
  

export function getToken() {
    return localStorage.getItem('token');
}