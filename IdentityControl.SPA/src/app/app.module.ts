import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routing.module';
import { SharedModule } from './modules/shared/shared.module';
import { NavbarComponent } from './components/navbar/navbar.component';
import { NavbarItemComponent } from './components/navbar/navbar-item/navbar-item.component';
import { NgxPermissionsModule } from 'ngx-permissions';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthModule } from './core/auth/auth.module';
import { SpinnerOverlayComponent } from './modules/shared/loading/loading-dialog/loading-dialog.component';
import { SpinnerOverlayService } from './modules/shared/loading/loading-dialog.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ErrorHandlerModule } from './core/errors/error-handler.module';
import { ToastrModule } from 'ngx-toastr';
import { DialogService } from './services/dialog.service';
import { NavbarSubitemComponent } from './components/navbar/navbar-subitem/navbar-subitem.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { HeaderComponent } from './components/header/header.component';
import { MatButtonModule } from '@angular/material/button';
import { MatRippleModule } from '@angular/material/core';
import { SearchService } from './services/search.service';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    NavbarComponent,
    NavbarItemComponent,
    NavbarSubitemComponent,
    SpinnerOverlayComponent
  ],
  imports: [
    BrowserAnimationsModule,
    AppRoutingModule,
    AuthModule.forRoot(),
    NgxPermissionsModule.forRoot(),
    MatProgressSpinnerModule,
    ErrorHandlerModule,
    ToastrModule.forRoot({
      timeOut: 4000,
      preventDuplicates: true,
      countDuplicates: true,
      resetTimeoutOnDuplicate: true,
      maxOpened: 3,
      progressBar: true,
      closeButton: true,
      extendedTimeOut: 1000,
      progressAnimation: 'decreasing'
    }),
    MatTooltipModule,
    MatButtonModule,
    MatRippleModule
  ],
  exports: [SharedModule],
  bootstrap: [AppComponent],
  providers: [SpinnerOverlayService, DialogService, SearchService]
})
export class AppModule {}
