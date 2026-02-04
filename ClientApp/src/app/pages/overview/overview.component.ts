import { Component, inject } from '@angular/core';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { PortfolioApiService } from '../../api/portfolio-api.service';
import type { PortfolioSummary } from '../../api/portfolio.models';
import { isError, isLoading, isReady, type ViewState } from '../../shared/view-state';
import { catchError, map, of, startWith } from 'rxjs';

@Component({
  selector: 'app-overview',
  imports: [AsyncPipe, NgIf, NgFor],
  templateUrl: './overview.component.html',
  styleUrl: './overview.component.scss'
})
export class OverviewComponent {
  private readonly api = inject(PortfolioApiService);

  protected readonly isLoading = isLoading;
  protected readonly isReady = isReady;
  protected readonly isError = isError;

  protected readonly vm$ = this.api.getSummary().pipe(
    map((data) => ({ status: 'ready', data }) satisfies ViewState<PortfolioSummary>),
    startWith({ status: 'loading' } satisfies ViewState<PortfolioSummary>),
    catchError(() =>
      of({
        status: 'error',
        message: 'Failed to load data. Is the backend running?'
      } satisfies ViewState<PortfolioSummary>)
    )
  );
}
