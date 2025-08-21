import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ExpenseService, Person, Category, CreateExpense } from '../../services/expense.service';

// ng-zorro-antd Components
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzAlertModule } from 'ng-zorro-antd/alert';

@Component({
  selector: 'app-expense-form',
  templateUrl: './expense-form.component.html',
  styleUrls: ['./expense-form.component.css'],
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    NzInputModule,
    NzInputNumberModule,
    NzDatePickerModule,
    NzSelectModule,
    NzButtonModule,
    NzCardModule,
    NzMessageModule,
    NzFormModule,
    NzIconModule,
    NzDividerModule,
    NzSpaceModule,
    NzTypographyModule,
    NzAlertModule
  ]
})
export class ExpenseFormComponent implements OnInit {
  @Output() expenseAdded = new EventEmitter<void>();
  
  expenseForm: FormGroup;
  people: Person[] = [];
  categories: Category[] = [];
  loading = false;
  error = '';
  success = '';

  constructor(
    private fb: FormBuilder,
    private expenseService: ExpenseService
  ) {
    this.expenseForm = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(100)]],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      date: ['', Validators.required],
      notes: ['', Validators.maxLength(500)],
      personId: ['', Validators.required],
      categoryIds: [[], [Validators.required, Validators.minLength(1)]]
    });
  }

  ngOnInit(): void {
    this.loadPeople();
    this.loadCategories();
    this.setDefaultDate();
    
    // Add a small delay to ensure form is fully initialized
    setTimeout(() => {
      this.validateForm();
    }, 100);
  }

  private setDefaultDate(): void {
    const today = new Date();
    this.expenseForm.patchValue({ date: today });
    // Trigger validation after setting default date
    this.expenseForm.get('date')?.updateValueAndValidity();
  }

  loadPeople(): void {
    this.expenseService.getPeople().subscribe({
      next: (people) => {
        this.people = people;
      },
      error: (error) => {
        this.error = 'Failed to load people. Please try again.';
        console.error('Error loading people:', error);
      }
    });
  }

  loadCategories(): void {
    this.expenseService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        this.error = 'Failed to load categories. Please try again.';
        console.error('Error loading categories:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.expenseForm.valid) {
      this.loading = true;
      this.error = '';
      this.success = '';

      const formValue = this.expenseForm.value;
      const expense: CreateExpense = {
        Description: formValue.description,
        Amount: parseFloat(formValue.amount),
        Date: formValue.date instanceof Date ? formValue.date.toISOString().split('T')[0] : formValue.date,
        Notes: formValue.notes || undefined,
        PersonId: formValue.personId,
        CategoryIds: formValue.categoryIds
      };

      this.expenseService.createExpense(expense).subscribe({
        next: () => {
          this.success = 'Expense added successfully!';
          this.expenseForm.reset();
          this.setDefaultDate();
          this.expenseAdded.emit();
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to add expense. Please try again.';
          this.loading = false;
          console.error('Error creating expense:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.expenseForm.controls).forEach(key => {
      const control = this.expenseForm.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.expenseForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.expenseForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) return 'This field is required.';
      if (field.errors['maxlength']) return `Maximum length is ${field.errors['maxlength'].requiredLength} characters.`;
      if (field.errors['min']) return `Minimum value is ${field.errors['min'].min}.`;
      if (field.errors['minlength']) return `At least ${field.errors['minlength'].requiredLength} item(s) must be selected.`;
    }
    return '';
  }

  onCategoryChange(event: any): void {
    const selectedOptions = Array.from(event.target.selectedOptions, (option: any) => option.value);
    this.expenseForm.patchValue({ categoryIds: selectedOptions });
  }

  formatCurrency = (value: number): string => {
    return `$${value.toFixed(2)}`;
  }

  parseCurrency = (value: string): number => {
    return parseFloat(value.replace(/[^\d.-]/g, '')) || 0;
  }



  private validateForm(): void {
    console.log('Validating form...');
    this.expenseForm.updateValueAndValidity();
    
    // Log the status of each control
    Object.keys(this.expenseForm.controls).forEach(key => {
      const control = this.expenseForm.get(key);
      console.log(`Control ${key}:`, {
        value: control?.value,
        valid: control?.valid,
        errors: control?.errors,
        dirty: control?.dirty,
        touched: control?.touched
      });
    });
  }

  getAmountType(): string {
    const value = this.expenseForm.get('amount')?.value;
    return typeof value;
  }

  isFormValid(): boolean {
    const description = this.expenseForm.get('description')?.value;
    const amount = this.expenseForm.get('amount')?.value;
    const date = this.expenseForm.get('date')?.value;
    const personId = this.expenseForm.get('personId')?.value;
    const categoryIds = this.expenseForm.get('categoryIds')?.value;
    
    // Check amount field specifically
    const amountControl = this.expenseForm.get('amount');
    const amountValid = amountControl?.valid;
    const amountErrors = amountControl?.errors;
    
    console.log('Form validation check:', {
      description,
      amount,
      date,
      personId,
      categoryIds,
      formValid: this.expenseForm.valid,
      amountValid,
      amountErrors,
      amountValue: amount,
      amountType: typeof amount
    });
    
    return this.expenseForm.valid && 
           description && 
           amount !== null && amount !== undefined && amount >= 0.01 && 
           date && 
           personId && 
           categoryIds?.length > 0;
  }
}
