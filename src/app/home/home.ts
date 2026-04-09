import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../services/auth';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit {
  slots: any[] = [];
  loading = false;
  errorMessage = '';
  bookingLoadingId: number | null = null;
  username = '';

  private activeRequests = 0;

  constructor(
    public router: Router,
    private http: HttpClient,
    private authService: AuthService
    
  ) {
    this.username = this.authService.getUsername();

    this.authService.username$.subscribe((name) => {
      this.username = name;
    });

    this.loadSlots();
  }

  ngOnInit(): void {
    

    this.loadSlots();
  }

  loadSlots(): void {
    this.activeRequests++;
    this.loading = true;
    this.errorMessage = '';

    this.http.get<any[]>('https://localhost:58811/api/Slots/available')
      .pipe(
        finalize(() => {
          this.activeRequests--;

          if (this.activeRequests <= 0) {
            this.activeRequests = 0;
            this.loading = false;
          }
        })
      )
      .subscribe({
        next: (data) => {
          console.log('AVAILABLE SLOTS = ', data);
          this.slots = Array.isArray(data) ? data : [];
        },
        error: (err) => {
          console.error('LOAD SLOTS ERROR = ', err);
          this.errorMessage = 'Failed to load appointments.';
          this.slots = [];
        }
      });
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  book(slotId: number): void {
    const token = this.authService.getToken();

    if (!token) {
      localStorage.setItem('selectedSlotId', slotId.toString());
      this.router.navigate(['/login']);
      return;
    }

    this.bookingLoadingId = slotId;

    const body = {
      timeSlotId: slotId,
      notes: 'Booked from Angular UI'
    };

    this.http.post<any>(
      'https://localhost:58811/api/Appointments/book',
      body,
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    ).subscribe({
      next: (res) => {
        this.bookingLoadingId = null;
        localStorage.removeItem('selectedSlotId');
        alert(res?.message || 'Appointment booked successfully.');
        this.loadSlots();
      },
      error: (err) => {
        this.bookingLoadingId = null;

        const apiMessage =
          err?.error?.message ||
          err?.error?.title ||
          err?.error ||
          err?.message ||
          'Booking failed.';

        alert(apiMessage);
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.username = '';
    this.loadSlots();
  }
}