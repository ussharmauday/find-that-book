import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError, timeout } from 'rxjs';
import { SearchResponse } from '../models/search.models';

@Injectable({ providedIn: 'root' })
export class BookSearchService {
  private apiUrl = 'https://localhost:7243/api/book/search';


  constructor(private http: HttpClient) {}

  search(query: string): Observable<SearchResponse> {
    return this.http.post<SearchResponse>(`${this.apiUrl}`, {
      'query': query
    }).pipe(
      timeout(10000),
      catchError(error => {
        return throwError(() => error);
      }));
  }
}
