import {Component, OnInit} from '@angular/core';
import {AuthService} from './core/auth/auth.service';
import {SignalRService} from './services/signal-r.service';

@Component({
  selector: 'app-root',
  styleUrls: ['./app.component.scss'],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'Identity Control';
  searchInput: Event;

  constructor(
    public signalRService: SignalRService,
    private authService: AuthService
  ) {
    this.authService.runInitialLoginSequence();
  }

  ngOnInit(): void {
    if (this.authService.userId) {
      this.signalRService.startConnection();
      this.signalRService.addEventsListener();
    }
  }

  onHeaderSearchInput(event: Event) {
    this.searchInput = event;
  }
}
