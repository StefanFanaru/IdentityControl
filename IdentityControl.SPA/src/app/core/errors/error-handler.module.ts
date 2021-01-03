import { ErrorHandler, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { GlobalErrorHandler } from './global-error-handler';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { CustomHttpInterceptor } from './http-interceptor';

@NgModule({
  declarations: [],
  imports: [CommonModule, MatDialogModule],

  providers: [
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CustomHttpInterceptor,
      multi: true
    }
  ]
})
export class ErrorHandlerModule {
}
