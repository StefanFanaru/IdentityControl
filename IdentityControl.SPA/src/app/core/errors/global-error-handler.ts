import { ErrorHandler, Injectable } from '@angular/core';
import { DialogService } from '../../services/dialog.service';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private dialogService: DialogService) {
  }

  handleError(error: Error) {
    this.dialogService.openErrorDialog(
      error.stack || 'Undefined client error',
      'TypeScript'
    );
    console.error(error);
  }
}
