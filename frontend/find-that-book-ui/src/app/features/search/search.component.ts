import { Component, ChangeDetectorRef } from '@angular/core';
import { BookSearchService } from '../../core/services/book-search.service';
import { BookCandidate } from '../../core/models/search.models';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
  standalone: true,
  imports: [FormsModule]
})
export class SearchComponent {
  query = '';
  loading = false;
  error: string | null = null;
  results: BookCandidate[] = [];
  hasSearched = false;
  constructor(private searchService: BookSearchService, private cdr: ChangeDetectorRef) {}

  search() {
    if (!this.query.trim()) return;

    this.hasSearched = true;
    this.loading = true;
    this.error = null;
    this.results = [];

    this.searchService.search(this.query)
      .pipe(finalize(() => {this.loading = false; console.log(this.loading);}))
      .subscribe({
        next: response => {          
          this.results = response.results;
          if (this.results!=null && this.results.length === 0) {
            this.error = null; 
          }
          console.log(response);
          this.loading=false;
          this.cdr.detectChanges();
        },
        error: err => {
          if (err.status === 0) {
            this.error = 'Service unavailable. Please try again.';
          } else if (err.status >= 500) {
            this.error = 'Server error. Please try again later.';
          } else {
            this.error = err.error?.title ?? 'Unexpected error occurred.';
          }

          this.results = [];
          this.hasSearched = true;
          this.loading = false;
          this.cdr.detectChanges();
        }
      });
  }
}
