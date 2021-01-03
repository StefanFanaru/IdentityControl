import {Injectable} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {EventDto} from '../modules/shared/event';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  constructor(private toastr: ToastrService) {}

  showToaster(event: EventDto) {
    switch (event.Type) {
      case 'Info':
        this.toastr.info(event.Message, event.Title);
        break;
      case 'Warning':
        this.toastr.warning(event.Message, event.Title);
        break;
      case 'Error':
        this.toastr.error(event.Message, event.Title);
        break;
      case 'Success':
        this.toastr.success(event.Message, event.Title);
        break;
      default:
        throw new Error('Undefined event type detected');
    }
  }
}
