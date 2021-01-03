import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BaseIdentityModel } from '../../../models/baseIdentityModel';

@Injectable()
export class FormDialogService<TItem extends BaseIdentityModel> {
  private opened = false;

  constructor(private dialog: MatDialog) {
  }

  openErrorDialog(item: TItem): void {
    // if (!this.opened) {
    //   this.opened = true;
    //   // console.log({ message, status, json })
    //   const dialogRef = this.dialog.open(FormDialogComponent, {
    //     data: {content, type, json, status},
    //     width: type === 'Exception' ? '1200px' : '540px',
    //     maxHeight: '100%',
    //     maxWidth: '100%',
    //     disableClose: false,
    //     hasBackdrop: true
    //   });
    //
    //   dialogRef.afterClosed().subscribe(() => {
    //     this.opened = false;
    //   });
    // }
  }
}
