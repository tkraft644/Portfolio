import { Component, inject } from '@angular/core';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { PortfolioApiService } from '../../api/portfolio-api.service';
import type { ProjectsModel } from '../../api/portfolio.models';
import { isError, isLoading, isReady, type ViewState } from '../../shared/view-state';
import { catchError, map, of, startWith } from 'rxjs';

@Component({
  selector: 'app-projects',
  imports: [AsyncPipe, NgIf, NgFor],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent {
  private readonly api = inject(PortfolioApiService);

  protected readonly isLoading = isLoading;
  protected readonly isReady = isReady;
  protected readonly isError = isError;

  protected readonly vm$ = this.api.getProjects().pipe(
    map((data) => ({ status: 'ready', data }) satisfies ViewState<ProjectsModel>),
    startWith({ status: 'loading' } satisfies ViewState<ProjectsModel>),
    catchError(() =>
      of({
        status: 'error',
        message: 'Failed to load data. Is the backend running?'
      } satisfies ViewState<ProjectsModel>)
    )
  );
}
