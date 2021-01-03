import {
  Component,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {filter} from 'rxjs/operators';
import {SearchService} from '../../services/search.service';

@Component({
  selector: 'identity-control-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  @Output() searchInput = new EventEmitter<Event>();
  @ViewChild('searchInput') searchInputRef: ElementRef;
  searchTerm: string;
  searchHasValue: boolean;
  searchPlaceholderText = 'Search for anything ...';
  searchPlaceholder = this.searchPlaceholderText;
  showSearchInputBorder: any;

  constructor(
    public router: Router,
    public route: ActivatedRoute,
    public searchService: SearchService
  ) {
    console.log('header constructor');
    router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        let queryTerm = this.route.snapshot.queryParams.searchTerm;
        if (queryTerm !== undefined) {
          this.searchInputRef.nativeElement.value = queryTerm;
          this.searchHasValue = true;
        } else {
          this.searchInputRef.nativeElement.value = '';
        }
        if (!queryTerm) {
          this.onSearchBlur();
        }
      });
  }

  ngOnInit(): void {}

  onClick(url: string) {
    this.router.navigateByUrl(url);
  }

  onSearchInput(value: string) {
    console.log('input event: ' + value);
    if (value === '') {
      value = null;
    }
    this.searchService.writeInput(value);
  }

  clearSearchInput() {
    this.searchService.writeInput(null);
    this.searchInputRef.nativeElement.value = '';
    this.onSearchBlur();
  }

  onSearchBlur() {
    if (!this.searchInputRef.nativeElement.value) {
      this.searchHasValue = false;
      this.searchPlaceholder = this.searchPlaceholderText;
    }
    this.showSearchInputBorder = false;
  }

  onSearchInputFocus() {
    this.searchHasValue = true;
    this.showSearchInputBorder = true;
    this.searchPlaceholder = '';
  }
}
