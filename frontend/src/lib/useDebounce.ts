import { useEffect, useState } from 'react';

export function useDebounce<T>(valor: T, atrasoMs = 400): T {
  const [valorDebounced, setValorDebounced] = useState(valor);

  useEffect(() => {
    const timer = setTimeout(() => setValorDebounced(valor), atrasoMs);
    return () => clearTimeout(timer);
  }, [valor, atrasoMs]);

  return valorDebounced;
}
