import { Routes } from "@angular/router";
import { AuthGuard } from "./guards/auth.guard";
import { AccessPageComponent } from "./pages/access-page/access-page.component";
import { ChatPageComponent } from "./pages/chat-page/chat-page.component";  
import { RegisterPageComponent } from "./pages/register-page/register-page.component";

export const routes: Routes = [
   
    {
        path: '',
        component: AccessPageComponent, 
    },
    
    {
        path: 'register',
        component: RegisterPageComponent, 
    },  
   
    {
        path: '',
        component: ChatPageComponent,
        children: [
            {
                path: 'chat',
                canActivate: [AuthGuard],
                component: ChatPageComponent
            }
        ]
    },
    /*
    {
        path: '**',
        component: LoginComponent
    }*/
];