import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.html'
})
export class HomeComponent implements OnInit {

  slots: any[] = [];

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    this.http.get<any[]>('https://localhost:58811/api/Slots/available')
      .subscribe({
        next: (data) => {
          console.log('DATA = ', data);
          this.slots = data;
        },
        error: (err) => {
          console.error('ERROR = ', err);
        }
      });
  }

  book(slotId: number) {
    const token = localStorage.getItem('token');

    if (token) {
      this.router.navigate(['/book', slotId]);
    } else {
      this.router.navigate(['/login']);
    }
  }
}