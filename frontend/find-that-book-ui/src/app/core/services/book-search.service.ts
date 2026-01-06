import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, of, throwError, timeout } from 'rxjs';
import { BookCandidate } from '../models/search.models';
import { delay, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class BookSearchService {
  private apiUrl = 'https://localhost:7243/api/book/search';


  constructor(private http: HttpClient) {}

  search(query: string): Observable<BookCandidate[]> {
    return this.http.post<BookCandidate[]>(`${this.apiUrl}`, {
      'query': query
    }).pipe(
      catchError(error => {
        return throwError(() => error);
      }));
  }
  //   search(query: string): Observable<BookCandidate> {
  //   const mockResponse = [
  //     {
  //       title: 'To Kill a Mockingbird',
  //       authors: ['F Scott. Fitgerald'],
  //       contributors: [],
  //       first_publish_year: 1960,
  //       coverImageUrl: 'https://covers.openlibrary.org/b/id/14351077-M.jpg',
  //       explanation: 'The title "To Kill a Mockingbird"...'
  //     },
  //     {
  //       title: 'The Jungle',
  //       authors: ['Upton Sinclair'],
  //       contributors: [],
  //       first_publish_year: 1707,
  //       coverImageUrl: 'https://covers.openlibrary.org/b/id/-1-M.jpg',
  //       explanation: 'The title "The Jungle"...'
  //     }
  //   ];

  //   return of(mockResponse).pipe(
  //     delay(500), // simulate HTTP latency
  //     map(items => ({
  //       results: items.map(item => ({
  //         title: item.title,
  //         primaryAuthors: item.authors,
  //         firstPublishYear: item.first_publish_year,
  //         coverImageUrl: item.coverImageUrl,
  //         explanation: item.explanation,
  //         workUrl: 'https://openlibrary.org' // mock URL
  //       }))
  //     }))
  //   );
  // }

}
