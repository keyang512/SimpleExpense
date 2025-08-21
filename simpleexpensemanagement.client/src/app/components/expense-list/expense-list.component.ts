import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ExpenseService, Expense } from '../../services/expense.service';

// ng-zorro-antd Components
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzAlertModule } from 'ng-zorro-antd/alert';

@Component({
  selector: 'app-expense-list',
  templateUrl: './expense-list.component.html',
  styleUrls: ['./expense-list.component.css'],
  standalone: true,
  imports: [
    CommonModule, 
    HttpClientModule,
    NzTableModule,
    NzButtonModule,
    NzCardModule,
    NzMessageModule,
    NzModalModule,
    NzIconModule,
    NzTagModule,
    NzPopconfirmModule,
    NzDividerModule,
    NzSpaceModule,
    NzTypographyModule,
    NzAlertModule
  ]
})
export class ExpenseListComponent implements OnInit {
  expenses: Expense[] = [];
  loading = false;
  error = '';

  constructor(
    private expenseService: ExpenseService
  ) { }

  ngOnInit(): void {
    this.loadExpenses();
  }

  loadExpenses(): void {
    this.loading = true;
    this.error = '';
    
    this.expenseService.getExpenses().subscribe({
      next: (expenses) => {
        this.expenses = expenses;
        this.loading = false;
      },
      error: (error) => {
        // Try simple method as fallback
        this.expenseService.getExpensesSimple().subscribe({
          next: (expenses) => {
            this.expenses = expenses;
            this.loading = false;
          },
          error: (simpleError) => {
            this.error = 'Failed to load expenses. Please try again.';
            this.loading = false;
          }
        });
      }
    });
  }

  deleteExpense(id: string): void {
    // Using ng-zorro-antd modal confirmation will be handled in template
    this.expenseService.deleteExpense(id).subscribe({
      next: () => {
        this.expenses = this.expenses.filter(e => e.Id !== id);
      },
      error: (error) => {
        this.error = 'Failed to delete expense. Please try again.';
      }
    });
  }

  formatDate(dateString: string | undefined | null): string {
    if (!dateString) {
      return 'N/A';
    }
    try {
      return new Date(dateString).toLocaleDateString();
    } catch (error) {
      return 'Invalid Date';
    }
  }

  formatAmount(amount: number | undefined | null): string {
    if (amount === undefined || amount === null) {
      return '$0.00';
    }
    return `$${amount.toFixed(2)}`;
  }

  getSeverity(amount: number | undefined | null): string {
    if (amount === undefined || amount === null) {
      return 'default';
    }
    if (amount > 100) return 'error';
    if (amount > 50) return 'warning';
    return 'success';
  }
}
