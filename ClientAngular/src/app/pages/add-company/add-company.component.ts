// src/app/pages/add-company/add-company.component.ts
import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-add-company',
    templateUrl: './add-company.component.html'
})
export class AddCompanyComponent {
    successMsg = '';
    errorMsg = '';
    loading = false;
    currentYear: number;

    companyForm = this.fb.group({
        name: ['', Validators.required],
        year: ['', [Validators.required, Validators.pattern('^[0-9]{4}$')]],
        price: ['', [Validators.required, Validators.min(1), Validators.pattern('^[0-9]+$')]]
    });

    constructor(private fb: FormBuilder, private http: HttpClient) {
        this.currentYear = new Date().getFullYear();
    }

    onSubmit() {
        if (this.companyForm.invalid) return;
        this.loading = true;

        const body = {
            name: this.companyForm.value.name,
            year: Number(this.companyForm.value.year),
            price: Number(this.companyForm.value.price)
        };

        this.http.post<any>('http://localhost:5000/api/companies', body)
            .subscribe({
                next: res => {
                    this.successMsg = res.message;
                    this.errorMsg = '';
                    this.companyForm.reset();
                    this.loading = false;
                },
                error: err => {
                    this.loading = false;
                    if (err.error && err.error.message) {
                        this.errorMsg = err.error.message;
                    } else {
                        this.errorMsg = 'Bir hata oluştu.';
                    }
                    this.successMsg = '';
                }
            });
    }

    formatPrice(event: any) {
        let value = event.target.value;
        const cleanedValue = value.replace(/[^0-9]/g, '');
        this.companyForm.get('price')?.setValue(cleanedValue, { emitEvent: false });
        const formattedValue = cleanedValue ? Number(cleanedValue).toLocaleString('tr-TR') : '';
        event.target.value = formattedValue;
    }
}
