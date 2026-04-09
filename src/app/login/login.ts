import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  username = '';
  password = '';
  loading = false;
  errorMessage = '';

  constructor(
    private http: HttpClient,
    private router: Router,
    private authService: AuthService
  ) {}

  onLogin(): void {
    this.loading = true;
    this.errorMessage = '';

    const body = {
      username: this.username,
      password: this.password
    };

    this.authService.login(body).subscribe({
      next: () => {
        const selectedSlotId = localStorage.getItem('selectedSlotId');

        if (!selectedSlotId) {
          this.loading = false;
          this.router.navigate(['/']);
          return;
        }

        const token = this.authService.getToken();

        const bookingBody = {
          timeSlotId: Number(selectedSlotId),
          notes: 'Booked after login from Angular UI'
        };

        this.http.post<any>(
          'https://localhost:58811/api/Appointments/book',
          bookingBody,
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          }
        ).subscribe({
          next: (bookRes) => {
            localStorage.removeItem('selectedSlotId');
            this.loading = false;
            alert(bookRes?.message || 'Appointment booked successfully.');
            this.router.navigate(['/']);
          },
          error: (bookErr) => {
            this.loading = false;

            const apiMessage =
              bookErr?.error?.message ||
              bookErr?.error?.title ||
              bookErr?.error ||
              bookErr?.message ||
              'Login successful, but booking failed.';

            alert(apiMessage);
            this.router.navigate(['/']);
          }
        });
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Invalid username or password.';
      }
    });
  }
}