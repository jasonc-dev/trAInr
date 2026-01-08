/**
 * LoadingState Component
 * Displays a loading spinner with optional message
 */

import React from 'react';
import { LoadingWrapper, Spinner, LoadingText } from './LoadingState.styles';

interface LoadingStateProps {
  message?: string;
}

export const LoadingState: React.FC<LoadingStateProps> = ({
  message = 'Loading...',
}) => {
  return (
    <LoadingWrapper>
      <Spinner />
      <LoadingText>{message}</LoadingText>
    </LoadingWrapper>
  );
};
