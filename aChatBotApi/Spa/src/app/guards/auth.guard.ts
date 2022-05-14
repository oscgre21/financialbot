import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthorizationServices } from '../services/auth/authorization.service';
 

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private authService: AuthorizationServices, private router: Router) { }

    canActivate(): boolean {
        const canActivate = this.authService.isAuthenticated();
        console.log("xx")
        if (!canActivate) {
            this.router.navigateByUrl('/');
        }
        
        return canActivate;
    }
}