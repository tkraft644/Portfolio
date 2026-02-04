export type ViewState<T> =
  | { status: 'loading' }
  | { status: 'error'; message: string }
  | { status: 'ready'; data: T };

export function isLoading<T>(state: ViewState<T>): state is { status: 'loading' } {
  return state.status === 'loading';
}

export function isError<T>(state: ViewState<T>): state is { status: 'error'; message: string } {
  return state.status === 'error';
}

export function isReady<T>(state: ViewState<T>): state is { status: 'ready'; data: T } {
  return state.status === 'ready';
}
