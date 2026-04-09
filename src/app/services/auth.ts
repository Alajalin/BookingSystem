import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:58811/api/Auth';

  private usernameSubject = new BehaviorSubject<string>(
    localStorage.getItem('username') || ''
  );

  username$ = this.usernameSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, data).pipe(
      tap((res) => {
        if (res.token) {
          localStorage.setItem('token', res.token);
        }

        if (res.username) {
          localStorage.setItem('username', res.username);
          this.usernameSubject.next(res.username);
        }

        if (res.role) {
          localStorage.setItem('role', res.role);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    localStorage.removeItem('selectedSlotId');
    this.usernameSubject.next('');
  }

  getToken(): string {
    return localStorage.getItem('token') || '';
  }

  getUsername(): string {
    return localStorage.getItem('username') || '';
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
}