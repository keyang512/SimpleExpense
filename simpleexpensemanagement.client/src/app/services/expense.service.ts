import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Person {
  id: string;
  name: string;
}

export interface Category {
  id: string;
  name: string;
}

export interface Expense {
  id: string;
  description: string;
  amount: number;
  date: string;
  notes?: string;
  person: Person;
  categories: Category[];
  createdAt: string;
  updatedAt?: string;
}

export interface CreateExpense {
  description: string;
  amount: number;
  date: string;
  notes?: string;
  personId: string;
  categoryIds: string[];
}

export interface UpdateExpense {
  description: string;
  amount: number;
  date: string;
  notes?: string;
  personId: string;
  categoryIds: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private expensesUrl = '/api/expenses';
  private peopleUrl = '/api/people';
  private categoriesUrl = '/api/categories';

  constructor(private http: HttpClient) { }

  // Expense operations
  getExpenses(): Observable<Expense[]> {
    return this.http.get<Expense[]>(this.expensesUrl);
  }

  getExpense(id: string): Observable<Expense> {
    return this.http.get<Expense>(`${this.expensesUrl}(${id})`);
  }

  createExpense(expense: CreateExpense): Observable<Expense> {
    return this.http.post<Expense>(this.expensesUrl, expense);
  }

  updateExpense(id: string, expense: UpdateExpense): Observable<void> {
    return this.http.put<void>(`${this.expensesUrl}(${id})`, expense);
  }

  deleteExpense(id: string): Observable<void> {
    return this.http.delete<void>(`${this.expensesUrl}(${id})`);
  }

  getExpensesSummary(): Observable<any> {
    return this.http.get<any>(`${this.expensesUrl}/summary`);
  }

  testConnection(): Observable<any> {
    return this.http.get<any>(`${this.expensesUrl}/test-connection`);
  }

  // People operations
  getPeople(): Observable<Person[]> {
    return this.http.get<Person[]>(this.peopleUrl);
  }

  getPerson(id: string): Observable<Person> {
    return this.http.get<Person>(`${this.peopleUrl}(${id})`);
  }

  getPersonExpenses(personId: string): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.peopleUrl}(${personId})/expenses`);
  }

  getPersonExpensesCount(personId: string): Observable<number> {
    return this.http.get<number>(`${this.peopleUrl}(${personId})/expenses/$count`);
  }

  getPeopleSummary(): Observable<any> {
    return this.http.get<any>(`${this.peopleUrl}/summary`);
  }

  // Categories operations
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.categoriesUrl);
  }

  getCategory(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.categoriesUrl}(${id})`);
  }

  getCategoryExpenses(categoryId: string): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.categoriesUrl}(${categoryId})/expenses`);
  }

  getCategoryExpensesCount(categoryId: string): Observable<number> {
    return this.http.get<number>(`${this.categoriesUrl}(${categoryId})/expenses/$count`);
  }

  getCategoriesSummary(): Observable<any> {
    return this.http.get<any>(`${this.categoriesUrl}/summary`);
  }

  getPopularCategories(): Observable<any> {
    return this.http.get<any>(`${this.categoriesUrl}/popular`);
  }
}
