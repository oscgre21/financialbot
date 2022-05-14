import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { routes } from './routing';
import { AccessPageComponent } from './pages/access-page/access-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { AuthGuard } from './guards/auth.guard';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BearerTokenInterceptor } from 'src/helpers/bearer-token-interceptor';
import { getManUrl, getToken } from 'src/helpers/helpers';
import { JwtModule } from '@auth0/angular-jwt';
import { ChatPageComponent } from './pages/chat-page/chat-page.component';  
 


@NgModule({
  declarations: [
    AppComponent,
    AccessPageComponent,
    RegisterPageComponent,
    ChatPageComponent,  
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(routes),
    JwtModule.forRoot({
      config: {
        tokenGetter: getToken
      }
    }),
  ],
  providers: [
    AuthGuard,
    { provide: HTTP_INTERCEPTORS, useClass: BearerTokenInterceptor, multi: true},
    { provide: 'BASE_URL', useFactory: getManUrl, deps: [] }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
