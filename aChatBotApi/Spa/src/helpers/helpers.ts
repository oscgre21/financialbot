
export function getManUrl() {
    return  document.getElementsByTagName('base')[0].href;
}
  

export function getToken() {
    return localStorage.getItem('token');
}