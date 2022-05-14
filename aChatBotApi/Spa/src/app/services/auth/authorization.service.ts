import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { GenericResponse } from 'src/app/models/generic-response'; 
import { UserData } from 'src/app/models/user-data';
import { UserLogin } from 'src/app/models/user-login';
import { UserRegister } from 'src/app/models/user-register';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationServices {

  constructor(private http: HttpClient , @Inject('BASE_URL') private baseUrl: string, private jwtHelper: JwtHelperService, private router: Router) { }

  registerUser(request: UserRegister) {
    return this.http.post<GenericResponse>(`${this.baseUrl}api/accounts/register`, request).toPromise();
  }

  login(request: UserLogin) {
    return this.http.post<GenericResponse>(`${this.baseUrl}api/accounts/login`, request).toPromise();
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['']);
  }

  getCurrentUser() {
      let token=localStorage.getItem('token');
      if(token==null){token="";}
    return this.jwtHelper.decodeToken<UserData>(token);
  }

  isAuthenticated() {
    let token=localStorage.getItem('token');

    if(token==null){token="";}   
    return !this.jwtHelper.isTokenExpired(token);
  }
}