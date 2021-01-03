/* tslint:disable:semicolon */
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { AuthService } from '../core/auth/auth.service';
import { environment } from '../../environments/environment';
import { EventService } from './event.service';
import { EventDto } from '../modules/shared/event';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  public userId: string;

  constructor(
    private authService: AuthService,
    private eventService: EventService
  ) {
  }

  private eventHubConnection: signalR.HubConnection;
  public startConnection = () => {
    this.eventHubConnection = new signalR.HubConnectionBuilder()
      .withUrl(
        environment.identityControlApi + `/event-hub?userId=${this.authService.userId}`
      )
      .build();
    this.eventHubConnection
      .start()
      .then(() => {
        // console.log('Connection started');
        this.eventHubConnection
          .invoke('GetConnectionId')
          .then(identifier => {
          });
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  };
  public addEventsListener = () => {
    this.eventHubConnection.on('socket', data => {
      let event: EventDto = JSON.parse(data);
      this.eventService.showToaster(event);
    });
  };
}
