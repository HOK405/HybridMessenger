import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { PaginationRequest } from '../Models/Requests/PaginationRequest';
import { TokenResponse } from '../Models/Responses/TokenResponse';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = environment.baseUrl;

  constructor(private http: HttpClient, private authService: AuthService) {}

  login(loginModel: any): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.baseUrl}User/login`, loginModel)
      .pipe(
        tap(tokenResponse => this.authService.login(tokenResponse)),
        catchError(this.handleError)
      );
  }

  register(registerModel: any): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.baseUrl}User/register`, registerModel)
      .pipe(
        tap(tokenResponse => this.authService.login(tokenResponse)),
        catchError(this.handleError)
      );
  }

  getUserMessages(query: PaginationRequest): Observable<any[]> {
    const token = localStorage.getItem('accessToken');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<any[]>(`${this.baseUrl}Message/get-user-messages`, query, { headers })
      .pipe(catchError(this.handleError));
  }

  searchUsers(searchModel: PaginationRequest): Observable<any[]> {
    return this.http.post<any[]>(`${this.baseUrl}User/get-paged`, searchModel)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      if (error.status === 400) {
        errorMessage = 'Invalid email or password.';
      } else {
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      }
    }
    return throwError(errorMessage);
  }
}
