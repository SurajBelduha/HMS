// Modern API Client with Multi-Tenant and Branch Header Interceptors

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7154/api';

class ApiClient {
  private tenantId: string = '44444444-4444-4444-4444-444444444444'; // ABC Healthcare
  private branchId: string = 'JPR-01';
  private authToken: string | null = null;

  public setTenant(tenantId: string) {
    this.tenantId = tenantId;
  }

  public setBranch(branchId: string) {
    this.branchId = branchId;
  }

  public setToken(token: string | null) {
    this.authToken = token;
  }

  private getHeaders(): HeadersInit {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      'X-Tenant-Id': this.tenantId,
      'X-Branch-Id': this.branchId,
    };

    if (this.authToken) {
      headers['Authorization'] = `Bearer ${this.authToken}`;
    }

    return headers;
  }

  public async get<T>(endpoint: string): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'GET',
      headers: this.getHeaders(),
    });
    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }
    return response.json();
  }

  public async post<T>(endpoint: string, data: unknown): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }
    return response.json();
  }
}

export const api = new ApiClient();

