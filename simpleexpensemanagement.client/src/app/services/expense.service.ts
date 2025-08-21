import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface Person {
  Id: string;
  Name: string;
}

export interface Category {
  Id: string;
  Name: string;
}

export interface Expense {
  Id: string;
  Description: string;
  Amount: number;
  Date: string;
  Notes?: string;
  Person: Person;
  Categories: Category[];
  CreatedAt: string;
  UpdatedAt?: string;
}

export interface CreateExpense {
  Description: string;
  Amount: number;
  Date: string;
  Notes?: string;
  PersonId: string;
  CategoryIds: string[];
}

export interface UpdateExpense {
  Description: string;
  Amount: number;
  Date: string;
  Notes?: string;
  PersonId: string;
  CategoryIds: string[];
}

export interface ODataResponse<T> {
  '@odata.context': string;
  value: T[];
}

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private expensesUrl = '/api/Expenses';
  private peopleUrl = '/api/People';
  private categoriesUrl = '/api/Categories';

  constructor(private http: HttpClient) { }

  // Expense operations
  getExpenses(): Observable<Expense[]> {
    // Try different OData expand syntax
    const url = `${this.expensesUrl}?$expand=Categories,Person&$select=Id,Description,Amount,Date,Notes,Person,Categories,CreatedAt,UpdatedAt`;
    
    return this.http.get<any>(url).pipe(
      map(response => {
        // Handle different possible OData response structures
        let expenses: Expense[] = [];
        if (response && typeof response === 'object') {
          if (response.value && Array.isArray(response.value)) {
            expenses = response.value;
          } else if (Array.isArray(response)) {
            // Direct array response
            expenses = response;
          } else if (response['@odata.value'] && Array.isArray(response['@odata.value'])) {
            // Alternative OData structure
            expenses = response['@odata.value'];
          }
        }
        
        return expenses;
      })
    );
  }

  // Fallback method to get expenses without expand
  getExpensesSimple(): Observable<Expense[]> {
    const url = `${this.expensesUrl}`;
    
    return this.http.get<any>(url).pipe(
      map(response => {
        let expenses: Expense[] = [];
        if (response && typeof response === 'object') {
          if (response.value && Array.isArray(response.value)) {
            expenses = response.value;
          } else if (Array.isArray(response)) {
            expenses = response;
          }
        }
        return expenses;
      })
    );
  }

  getExpense(id: string): Observable<Expense> {
    return this.http.get<Expense>(`${this.expensesUrl}/${id}`);
  }

  createExpense(expense: CreateExpense): Observable<Expense> {
    return this.http.post<Expense>(this.expensesUrl, expense);
  }

  updateExpense(id: string, expense: UpdateExpense): Observable<void> {
    return this.http.put<void>(`${this.expensesUrl}/${id}`, expense);
  }

  deleteExpense(id: string): Observable<void> {
    return this.http.delete<void>(`${this.expensesUrl}/${id}`);
  }

  getExpensesSummary(): Observable<any> {
    return this.http.get<any>(`${this.expensesUrl}/summary`);
  }

  // People operations
  getPeople(): Observable<Person[]> {
    const url = `${this.peopleUrl}`;
    
    return this.http.get<any>(url).pipe(
      map(response => {
        // Handle different possible OData response structures
        let people: Person[] = [];
        if (response && typeof response === 'object') {
          if (response.value && Array.isArray(response.value)) {
            people = response.value;
          } else if (Array.isArray(response)) {
            // Direct array response
            people = response;
          } else if (response['@odata.value'] && Array.isArray(response['@odata.value'])) {
            // Alternative OData structure
            people = response['@odata.value'];
          }
        }
        
        return people;
      })
    );
  }

  getPerson(id: string): Observable<Person> {
    return this.http.get<Person>(`${this.peopleUrl}(${id})`);
  }

  getPersonExpenses(personId: string): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.peopleUrl}(${personId})/expenses`);
  }

  getPersonExpensesCount(personId: string): Observable<number> {
    return this.http.get<number>(`${this.peopleUrl}(${personId})/expenses/count`);
  }

  getPeopleSummary(): Observable<any> {
    return this.http.get<any>(`${this.peopleUrl}/summary`);
  }

  // Categories operations
  getCategories(): Observable<Category[]> {
    const url = `${this.categoriesUrl}`;
    
    return this.http.get<any>(url).pipe(
      map(response => {
        // Handle different possible OData response structures
        let categories: Category[] = [];
        if (response && typeof response === 'object') {
          if (response.value && Array.isArray(response.value)) {
            categories = response.value;
          } else if (Array.isArray(response)) {
            // Direct array response
            categories = response;
          } else if (response['@odata.value'] && Array.isArray(response['@odata.value'])) {
            // Alternative OData structure
            categories = response['@odata.value'];
          }
        }
        
        return categories;
      })
    );
  }

  getCategory(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.categoriesUrl}(${id})`);
  }

  getCategoryExpenses(categoryId: string): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.categoriesUrl}(${categoryId})/expenses`);
  }

  getCategoryExpensesCount(categoryId: string): Observable<number> {
    return this.http.get<number>(`${this.categoriesUrl}(${categoryId})/expenses/count`);
  }

  getCategoriesSummary(): Observable<any> {
    return this.http.get<any>(`${this.categoriesUrl}/summary`);
  }

  getPopularCategories(): Observable<any> {
    return this.http.get<any>(`${this.categoriesUrl}/popular`);
  }
}
