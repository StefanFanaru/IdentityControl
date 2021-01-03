import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable, Subject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private searchInputEvent: Subject<string> = new BehaviorSubject<string>('');

  get $searchInput(): Subject<string> {
    return this.searchInputEvent;
  }

  writeInput(value: string) {
    this.searchInputEvent.next(value);
  }

  constructor() {}
}
