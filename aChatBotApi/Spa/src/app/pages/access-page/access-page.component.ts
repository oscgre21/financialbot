import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { GenericResponse } from 'src/app/models/generic-response';
import { UserLogin } from 'src/app/models/user-login';
import { AuthorizationServices } from 'src/app/services/auth/authorization.service';

@Component({
  selector: 'app-access-page',
  templateUrl: './access-page.component.html',
  styleUrls: ['./access-page.component.css']
})
export class AccessPageComponent implements OnInit {
  loginForm!: FormGroup;
 
  constructor(private router: Router, private authService: AuthorizationServices) { }

  ngOnInit() {
 
    this.buildForm();
  }

  private buildForm() {
    this.loginForm = new FormGroup({
      userName: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required])
    });
  }

  validateControl(controlName: string) {
    return this.loginForm.controls[controlName].invalid && this.loginForm.controls[controlName].touched
  }

  hasError(controlName: string, errorName: string) {
    return this.loginForm.controls[controlName].hasError(errorName)
  }
 

  loginUser(loginFormValue:any) {
   // this.notificationService.showLoading();
    const login = {... loginFormValue };
    const userForAuth: UserLogin = {
      username: login.userName,
      password: login.password
    };

    this.authService.login(userForAuth).then((response: any) => {
      localStorage.setItem("token", response.token);
      
      this.router.navigateByUrl('/chat');
    }).catch((error: HttpErrorResponse) => {
      alert(error.error.errorMessage); 
    });
  }

}
