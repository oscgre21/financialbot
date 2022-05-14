import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router'; 
import { UserRegister } from 'src/app/models/user-register';
import { AuthorizationServices } from 'src/app/services/auth/authorization.service';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent implements OnInit {
  public registerForm!: FormGroup;
  
  constructor(private authService: AuthorizationServices, private router: Router ) { }

  ngOnInit() {
    this.buildForm();
   
  }

  private buildForm() {
    this.registerForm = new FormGroup({
      userName: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
      confirm: new FormControl('', [Validators.required])
    });
  }

  validateControl(controlName: string) {
    return this.registerForm.controls[controlName].invalid && this.registerForm.controls[controlName].touched
  }

  hasError(controlName: string, errorName: string) {
    return this.registerForm.controls[controlName].hasError(errorName)
  }
 

  registerUser(registerFormValue:any) {
 
    const formValues = { ...registerFormValue };
    const user: UserRegister = {
      username: formValues.userName,
      password: formValues.password,
      confirmpassword: formValues.confirm
    };
    this.authService.registerUser(user).then(() => {
      alert('Registered successfully!');
      this.router.navigateByUrl('/');
    }).catch((error: HttpErrorResponse) => {
      
      if (error.error.errors.ConfirmPassword){
        alert(error.error.errors.ConfirmPassword[0]);
      }else{
        alert(error.error.errors);
      } 
    });
  }
}
