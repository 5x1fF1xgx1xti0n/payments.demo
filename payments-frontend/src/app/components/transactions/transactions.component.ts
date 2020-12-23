import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Transaction } from 'src/app/models/transaction.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent implements OnInit {

  public role: string | null;
  public userId: number | null;
  public transactions!: Array<Transaction>; 
  public inputId!: number;
  public totalCommission!: Observable<number>;

  constructor(
    private _http: HttpClient,
    private _router: Router
  ) { 
    this.role = localStorage.getItem('role');
    this.userId = Number(localStorage.getItem('id'));
    if (!this.role){
      _router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    if (this.role == "Admin"){
      this.getAllTransactions();
      this.getTotalCommission();
    } else if (this.role == "User") {
      this.getUserTransactions(this.userId);
    }
  }

  private getAllTransactions(){
    this._http.get("https://localhost:44309/api/payments/transactions")
      .subscribe(data => {
        this.transactions = data as Array<Transaction>;
      })
  }

  getUserTransactions(id: number | null) {
    this._http.get(`https://localhost:44309/api/payments/transactions/${id}`)
      .subscribe(data => {
        this.transactions = data as Array<Transaction>;
      })
  }

  getTotalCommission() {
    this.totalCommission = this._http.get("https://localhost:44309/api/payments/commission/total") as Observable<number>;
  }
}
