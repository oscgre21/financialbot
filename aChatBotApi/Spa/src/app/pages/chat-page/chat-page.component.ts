import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthorizationServices } from 'src/app/services/auth/authorization.service';
import { HubServices } from 'src/app/services/hub/hub.service';
import { HubGroup } from './models/hub.group';
import { HubMessage } from './models/hub.message';
import { HubUsers } from './models/hub.users';

@Component({
  selector: 'app-chat-page',
  templateUrl: './chat-page.component.html',
  styleUrls: ['./chat-page.component.css']
})
export class ChatPageComponent implements OnInit {
  @HostListener('window:unload', ['$event']) unloadHandler(event:any) {
    this.disconnectCurrentUser();
  };
  @ViewChild('panel', { static: false }) private chatElement!: ElementRef;
  connectedUsers: HubUsers[] = [];
  connectedUsersSubscription!: Subscription;
  actualMessages: HubMessage[] = [];
  actualFilterMessages: HubMessage[] = [];
  actualGroup: HubGroup[] = [];
  actualGroupSuscription!: Subscription;
  actualMessagesSubscription!: Subscription;
  newMessageSubscription!: Subscription;
  actualUserName!: string;
  message = new FormControl('');
  selectedGroup: string = 'Global';

  constructor(private hub: HubServices, private authService: AuthorizationServices, private router: Router) {}

  ngOnInit() {
    if (!this.authService.isAuthenticated()) {
      this.router.navigateByUrl('/');
    }
    this.actualUserName = this.authService.getCurrentUser().username;
    this.suscribeToEvents();
   
  }

  filterMessages(){
    this.actualFilterMessages =  this.actualMessages.filter(x=>x.group==this.selectedGroup);
    this.chatScrollToBottom();
  }
  suscribeToEvents() {
    this.connectedUsersSubscription = this.hub.onlineUsers.subscribe((HubUsers: HubUsers[]) => {
      if (HubUsers !== undefined) {
        this.connectedUsers = HubUsers;
      }
    });

    this.actualMessagesSubscription = this.hub.actualMessages.subscribe((actualMessages: HubMessage[]) => {
      console.log(actualMessages)
      if (actualMessages !== undefined) {
        this.actualMessages = actualMessages; 
        this.filterMessages();
        this.chatScrollToBottom();
      }
    });
    this.actualGroupSuscription = this.hub.actualGroup.subscribe((groups: HubGroup[]) => {
      if (groups !== undefined) {
        this.actualGroup = groups; 
      }
    });

    this.newMessageSubscription = this.hub.newMessage.subscribe((newMessage: HubMessage) => {
    
      if (newMessage !== undefined) { 
        this.actualMessages.push(newMessage);
        this.filterMessages();
        this.chatScrollToBottom();

        if (newMessage.username === "#BOT") {
          this.hub.saveBotMessage(newMessage);
        }
      }
    });
  }

  addNewGroup(){
    let groupName = prompt("Please enter the name of the group"); 
    if (groupName != null) {
      this.hub.addNewGroup(groupName);
    }
  }
  private chatScrollToBottom() {
    setTimeout(() => {
      this.chatElement.nativeElement.scrollTop = this.chatElement.nativeElement.scrollHeight;
    }, 100);
  }

  checkIsEnterKey(event:any) {
    const enterKeyCode = 13;
    if (event.keyCode === enterKeyCode) {
      this.send();
    }
  }

  getStyleClassByUserName(userName: string,category:string) {
   
    if (userName === "#BOT" && category=='bot') { 
      return true;
    } else if (userName === this.actualUserName && category=='me') {
      return true
    } else if (userName !== this.actualUserName && category=='other' && userName !== "#BOT") {
      return true;
    }

    return false;
  }

  send() {
    if (this.message.value.trim() === "") {
      return;
    }

    const newMessage: HubMessage = {
      username: this.actualUserName,
      sendedDateUtc: new Date(),
      message: this.message.value,
      group : this.selectedGroup
    };

    this.hub.sendNewMessage(newMessage).then(() => {
      this.message.setValue("");
    }).catch((error: HttpErrorResponse) => {
      console.log(error);
    });
  } 
  disconnectCurrentUser() {
    this.connectedUsersSubscription.unsubscribe();
    this.actualMessagesSubscription.unsubscribe();
    this.hub.closeConnectionForCurrentClient();
  }
}
