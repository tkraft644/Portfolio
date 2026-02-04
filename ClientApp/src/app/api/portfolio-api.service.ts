import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import type { AboutModel, ContactResponse, PortfolioSummary, ProjectsModel } from './portfolio.models';

@Injectable({ providedIn: 'root' })
export class PortfolioApiService {
  constructor(private readonly http: HttpClient) {}

  getSummary() {
    return this.http.get<PortfolioSummary>('/api/portfolio/summary');
  }

  getAbout() {
    return this.http.get<AboutModel>('/api/portfolio/about');
  }

  getProjects() {
    return this.http.get<ProjectsModel>('/api/portfolio/projects');
  }

  getContact() {
    return this.http.get<ContactResponse>('/api/portfolio/contact');
  }
}
