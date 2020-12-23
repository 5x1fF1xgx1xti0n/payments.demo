import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { User } from 'src/app/models/user.model';
import { Sum } from 'src/app/models/sum.model';
import { PaymentWithCommission } from 'src/app/models/payment-with-commission.model';
import { CreateTransactionModel } from 'src/app/models/create-transaction.model';

@Component({
  selector: 'app-transfer',
  templateUrl: './transfer.component.html',
  styleUrls: ['./transfer.component.css']
})
export class TransferComponent implements OnInit {

  formGroup!: FormGroup;
  public userId: number | null;
  public user!: Observable<User>;
  public checked: boolean = false;
  public payment!: PaymentWithCommission;
  public success: boolean = false;
  
  constructor(
    private _http: HttpClient,
    private _router: Router
  ) { 
    this.userId = Number(localStorage.getItem('id'));
    if (!this.userId){
      _router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    this.getUser();
    this.formInitBuild();
  }

  check() {
    let sum = new Sum();
    sum.value = Number(this.formGroup.controls['amount'].value);
    this._http.post('https://localhost:44309/api/payments/commission', sum)
      .subscribe(data => {
        this.payment = data as PaymentWithCommission;
        this.checked = true;
      });
  }

  transfer() {
    let transaction = new CreateTransactionModel();
    this._http.get(`https://localhost:44309/api/payments/user/${this.userId}`)
      .subscribe(data => {
        transaction.fromAccountNumber = (data as User).account.number;
        transaction.rawSum = Number(this.formGroup.controls['amount'].value);
        transaction.toAccountNumber = this.formGroup.controls['to'].value;

        this._http.post('https://localhost:44309/api/payments/transaction', transaction)
          .subscribe(data => {
            this.success = true;
        });
      })
  }

  private getUser() {
    if (!this.userId) {
      return;
    }
    this.user = this._http.get(`https://localhost:44309/api/payments/user/${this.userId}`) as Observable<User>;
  }

  private formInitBuild(data: any = {}): void {
    this.formGroup = new FormGroup({
      'from': new FormControl('', [Validators.required]),
      'to': new FormControl('', [Validators.required]),
      'amount': new FormControl('', [Validators.required]),
    });
  }
}
