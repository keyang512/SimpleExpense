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
        this.error = 'Failed to load expenses. Please try again.';
        this.loading = false;
        console.error('Error loading expenses:', error);
      }
    });
  }

  deleteExpense(id: string): void {
    // Using ng-zorro-antd modal confirmation will be handled in template
    this.expenseService.deleteExpense(id).subscribe({
      next: () => {
        this.expenses = this.expenses.filter(e => e.id !== id);
      },
      error: (error) => {
        this.error = 'Failed to delete expense. Please try again.';
        console.error('Error deleting expense:', error);
      }
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  formatAmount(amount: number): string {
    return `$${amount.toFixed(2)}`;
  }

  getSeverity(amount: number): string {
    if (amount > 100) return 'error';
    if (amount > 50) return 'warning';
    return 'success';
  }
}
