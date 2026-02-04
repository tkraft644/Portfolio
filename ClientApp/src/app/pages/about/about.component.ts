import { Component, inject } from '@angular/core';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { PortfolioApiService } from '../../api/portfolio-api.service';
import type { AboutModel } from '../../api/portfolio.models';
import { isError, isLoading, isReady, type ViewState } from '../../shared/view-state';
import { catchError, map, of, startWith } from 'rxjs';

@Component({
  selector: 'app-about',
  imports: [AsyncPipe, NgIf, NgFor],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss'
})
export class AboutComponent {
  private readonly api = inject(PortfolioApiService);

  protected readonly isLoading = isLoading;
  protected readonly isReady = isReady;
  protected readonly isError = isError;

  protected readonly vm$ = this.api.getAbout().pipe(
    map((data) => ({ status: 'ready', data }) satisfies ViewState<AboutModel>),
    startWith({ status: 'loading' } satisfies ViewState<AboutModel>),
    catchError(() =>
      of({
        status: 'error',
        message: 'Failed to load data. Is the backend running?'
      } satisfies ViewState<AboutModel>)
    )
  );
}
