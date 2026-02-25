import { zodResolver } from '@hookform/resolvers/zod';
import AddIcon from '@mui/icons-material/Add';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { z } from 'zod';
import EmptyState from '../components/EmptyState';
import ErrorState from '../components/ErrorState';
import LoadingSpinner from '../components/LoadingSpinner';
import { useAlertDefsList, useCreateAlertDef, useExecuteAlertDef } from '../hooks/useAlertDefs';

const createSchema = z.object({
  name: z.string().min(1, 'Name is required').max(200, 'Name must not exceed 200 characters'),
  awsAccountId: z.string().min(1, 'AWS Account ID is required'),
  maxBillAmount: z.coerce.number().int().positive('Must be greater than 0'),
  alertRecipientEmails: z.string().min(1, 'Recipient emails are required'),
});

type CreateFormData = z.infer<typeof createSchema>;

export default function AlertDefsPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(20);
  const [createOpen, setCreateOpen] = useState(false);

  const { data, isLoading, isError, refetch } = useAlertDefsList(page + 1, rowsPerPage);
  const createMutation = useCreateAlertDef();
  const executeMutation = useExecuteAlertDef();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateFormData>({ resolver: zodResolver(createSchema) });

  const onCreateSubmit = async (formData: CreateFormData) => {
    try {
      await createMutation.mutateAsync(formData);
      toast.success('Alert definition created');
      setCreateOpen(false);
      reset();
    } catch {
      toast.error('Failed to create alert definition');
    }
  };

  const handleExecute = async (e: React.MouseEvent, id: string) => {
    e.stopPropagation();
    try {
      await executeMutation.mutateAsync(id);
      toast.success('Alert executed');
    } catch {
      toast.error('Failed to execute alert');
    }
  };

  if (isLoading) return <LoadingSpinner />;
  if (isError) return <ErrorState message="Failed to load alert definitions." onRetry={refetch} />;

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" fontWeight={600}>
          Alert Definitions
        </Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
          Create Alert Def
        </Button>
      </Box>

      {data && data.items.length === 0 ? (
        <EmptyState message="No alert definitions yet. Create your first one!" />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Name</strong></TableCell>
                <TableCell><strong>AWS Account ID</strong></TableCell>
                <TableCell align="right"><strong>Max Amount</strong></TableCell>
                <TableCell><strong>Recipients</strong></TableCell>
                <TableCell align="center"><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data?.items.map((alertDef) => (
                <TableRow
                  key={alertDef.id}
                  hover
                  sx={{ cursor: 'pointer' }}
                  onClick={() => navigate(`/alert-defs/${alertDef.id}`)}
                >
                  <TableCell>{alertDef.name}</TableCell>
                  <TableCell>{alertDef.awsAccountId}</TableCell>
                  <TableCell align="right">${alertDef.maxBillAmount.toLocaleString()}</TableCell>
                  <TableCell>{alertDef.alertRecipientEmails}</TableCell>
                  <TableCell align="center">
                    <Tooltip title="Execute Alert">
                      <IconButton
                        size="small"
                        color="primary"
                        onClick={(e) => handleExecute(e, alertDef.id)}
                        disabled={executeMutation.isPending}
                      >
                        <PlayArrowIcon />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          {data && (
            <TablePagination
              component="div"
              count={data.total}
              page={page}
              onPageChange={(_, newPage) => setPage(newPage)}
              rowsPerPage={rowsPerPage}
              onRowsPerPageChange={(e) => {
                setRowsPerPage(parseInt(e.target.value, 10));
                setPage(0);
              }}
              rowsPerPageOptions={[10, 20, 50]}
            />
          )}
        </TableContainer>
      )}

      <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create Alert Definition</DialogTitle>
        <Box component="form" onSubmit={handleSubmit(onCreateSubmit)} noValidate>
          <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <TextField
              label="Name"
              fullWidth
              error={!!errors.name}
              helperText={errors.name?.message}
              {...register('name')}
            />
            <TextField
              label="AWS Account ID"
              fullWidth
              error={!!errors.awsAccountId}
              helperText={errors.awsAccountId?.message}
              {...register('awsAccountId')}
            />
            <TextField
              label="Max Bill Amount"
              type="number"
              fullWidth
              error={!!errors.maxBillAmount}
              helperText={errors.maxBillAmount?.message}
              {...register('maxBillAmount')}
            />
            <TextField
              label="Alert Recipient Emails"
              fullWidth
              error={!!errors.alertRecipientEmails}
              helperText={errors.alertRecipientEmails?.message}
              {...register('alertRecipientEmails')}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setCreateOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" disabled={createMutation.isPending}>
              {createMutation.isPending ? 'Creating...' : 'Create'}
            </Button>
          </DialogActions>
        </Box>
      </Dialog>
    </Box>
  );
}
