import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-offer',
    templateUrl: './offer.component.html'
})
export class OfferComponent implements OnInit {
    companies: any[] = [];
    loading = false;
    offerResult: any = null;
    errorMsg: string = '';
    successMsg: string = '';
    todayDate: string = new Date().toISOString().slice(0, 10);

    offerForm = this.fb.group({
        companyId: ['', Validators.required],
        startDate: ['', Validators.required],
        endDate: ['', Validators.required]
    }, { validators: this.dateRangeValidator });

    constructor(private fb: FormBuilder, private http: HttpClient) {}

    ngOnInit() {
        this.http.get<any>('http://localhost:5000/api/companies')
            .subscribe(data => this.companies = data.data);
    }

    dateRangeValidator(control: AbstractControl): ValidationErrors | null {
        const startDate = new Date(control.get('startDate')?.value);
        const endDate = new Date(control.get('endDate')?.value);

        if (startDate && endDate && startDate > endDate) {
            return { dateRangeInvalid: true };
        }
        return null;
    }

    onSubmit() {
        if (this.offerForm.invalid) {
            this.errorMsg = 'Lütfen tarih ve firma bilgilerini doğru girin.';
            this.successMsg = '';
            return;
        }

        this.loading = true;
        const body = this.offerForm.value;

        this.http.post<any>('http://localhost:5000/api/offers/calculate', body)
            .subscribe({
                next: res => {
                    this.offerResult = res;
                    this.loading = false;
                    this.successMsg = res.message;
                    this.errorMsg = '';
                },
                error: err => {
                    this.errorMsg = err.error?.message || 'Bir hata oluştu.';
                    this.successMsg = '';
                    this.loading = false;
                }
            });
    }
}
