import { Box, Typography } from '@mui/material';
import InboxIcon from '@mui/icons-material/Inbox';

interface EmptyStateProps {
  message?: string;
}

export default function EmptyState({ message = 'No items found.' }: EmptyStateProps) {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        py: 8,
        opacity: 0.6,
      }}
    >
      <InboxIcon sx={{ fontSize: 64, mb: 2 }} />
      <Typography variant="h6">{message}</Typography>
    </Box>
  );
}
