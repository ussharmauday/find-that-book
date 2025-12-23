import { Component, signal } from '@angular/core';
import { SearchComponent } from './features/search/search.component';

@Component({
  selector: 'app-root',
  imports: [SearchComponent],
  templateUrl: './app.html',
  standalone: true,
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('find-that-book-ui');
}
