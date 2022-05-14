import { HttpErrorResponse } from '@angular/common/http';
import { EventEmitter, Inject, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubGroup } from 'src/app/pages/chat-page/models/hub.group';
import { HubMessage } from 'src/app/pages/chat-page/models/hub.message';
import { HubUsers } from 'src/app/pages/chat-page/models/hub.users';
import { getToken } from 'src/helpers/helpers';
import { AuthorizationServices } from '../auth/authorization.service';
 

@Injectable({
    providedIn: 'root'
})
export class HubServices {
    private connection: signalR.HubConnection;
    onlineUsers = new EventEmitter<HubUsers[]>();
    actualMessages = new EventEmitter<HubMessage[]>();
    newMessage = new EventEmitter<HubMessage>();
    actualGroup = new EventEmitter<HubGroup[]>();

    constructor(
        @Inject('BASE_URL') private baseUrl: string, 
        private authService: AuthorizationServices) {
        const options: signalR.IHttpConnectionOptions = {
            accessTokenFactory: () => { 
                return ""+getToken();
            }
            };
        this.connection = new signalR.HubConnectionBuilder().withUrl(`${this.baseUrl}hub/chat`, options).build();
        this.startConnection();
     }

    private startConnection() {
        this.connection.serverTimeoutInMilliseconds = 36000000;
        this.connection.keepAliveIntervalInMilliseconds = 1800000;

        this.connection.start().then(() => {
            this.receiveonlineUsers();
            this.receiveactualMessages();
            this.receiveMessage();
            this.receiveGroup();
            this.addNewGroup("Global");
 
        }).catch((error: HttpErrorResponse) => {
        
        });
    }
    private receiveGroup() {
        this.connection.on("GroupChanged", (group: HubGroup[]) => {
            this.actualGroup.emit(group);
         });
      }
    private receiveMessage() {
      this.connection.on("NewMessage", (message: HubMessage) => {
          this.newMessage.emit(message);
       });
    }

    private receiveonlineUsers() {
        this.connection.on("UsersChanged", (response: HubUsers[]) => {
            
            this.onlineUsers.emit(response);
        });
    }

    private receiveactualMessages() {
        this.connection.on("actualMessages", (messages: HubMessage[]) => {
            this.actualMessages.emit(messages)
        });
    }

    closeConnectionForCurrentClient() {
        const userName = this.authService.getCurrentUser().username;
        this.connection.invoke("DisconnectUser", userName).then(() => {
            this.authService.logout();
        }).catch(() => {
          /*  this.toastr.error("An error occurred while loging you out.", "Error", {
                positionClass: 'toast-bottom-right'
            });*/
        });
    }

    sendNewMessage(message: HubMessage) {
        return this.connection.invoke("SendMessage", message);
    }
    addNewGroup(groupName: string) {
        return this.connection.invoke("AddNewGroup", groupName);
    }

    saveBotMessage(message: HubMessage) {
        return this.connection.invoke("SaveBotMessage", message);
    }
}