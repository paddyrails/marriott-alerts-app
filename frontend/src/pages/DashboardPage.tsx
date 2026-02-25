import { Box, Button, Card, CardContent, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export default function DashboardPage() {
  const { user } = useAuth();
  const navigate = useNavigate();

  return (
    <Box>
      <Typography variant="h4" fontWeight={600} mb={3}>
        Dashboard
      </Typography>
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Welcome{user?.name ? `, ${user.name}` : ''}!
          </Typography>
          <Typography variant="body1" color="text.secondary" mb={2}>
            Manage your AWS billing alert definitions from here.
          </Typography>
          <Button variant="contained" onClick={() => navigate('/alert-defs')}>
            View Alert Definitions
          </Button>
        </CardContent>
      </Card>
    </Box>
  );
}
