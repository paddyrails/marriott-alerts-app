import { zodResolver } from '@hookform/resolvers/zod';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import DeleteIcon from '@mui/icons-material/Delete';
import { Box, Button, Paper, TextField, Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { z } from 'zod';
import ConfirmDialog from '../components/ConfirmDialog';
import ErrorState from '../components/ErrorState';
import LoadingSpinner from '../components/LoadingSpinner';
import { useAlertDef, useDeleteAlertDef, useUpdateAlertDef } from '../hooks/useAlertDefs';

const schema = z.object({
  name: z.string().min(1, 'Name is required').max(200, 'Name must not exceed 200 characters'),
  awsAccountId: z.string().min(1, 'AWS Account ID is required'),
  maxBillAmount: z.coerce.number().int().positive('Must be greater than 0'),
  alertRecipientEmails: z.string().min(1, 'Recipient emails are required'),
});

type FormData = z.infer<typeof schema>;

export default function AlertDefDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [deleteOpen, setDeleteOpen] = useState(false);

  const { data: alertDef, isLoading, isError, refetch } = useAlertDef(id!);
  const updateMutation = useUpdateAlertDef(id!);
  const deleteMutation = useDeleteAlertDef();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isDirty },
  } = useForm<FormData>({ resolver: zodResolver(schema) });

  useEffect(() => {
    if (alertDef) {
      reset({
        name: alertDef.name,
        awsAccountId: alertDef.awsAccountId,
        maxBillAmount: alertDef.maxBillAmount,
        alertRecipientEmails: alertDef.alertRecipientEmails,
      });
    }
  }, [alertDef, reset]);

  const onSubmit = async (data: FormData) => {
    try {
      await updateMutation.mutateAsync(data);
      toast.success('Alert definition updated');
    } catch {
      toast.error('Failed to update alert definition');
    }
  };

  const handleDelete = async () => {
    try {
      await deleteMutation.mutateAsync(id!);
      toast.success('Alert definition deleted');
      navigate('/alert-defs', { replace: true });
    } catch {
      toast.error('Failed to delete alert definition');
    }
  };

  if (isLoading) return <LoadingSpinner />;
  if (isError) return <ErrorState message="Failed to load alert definition." onRetry={refetch} />;
  if (!alertDef) return <ErrorState message="Alert definition not found." />;

  return (
    <Box>
      <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/alert-defs')} sx={{ mb: 2 }}>
        Back to List
      </Button>

      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" fontWeight={600}>
          Edit Alert Definition
        </Typography>
        <Button
          variant="outlined"
          color="error"
          startIcon={<DeleteIcon />}
          onClick={() => setDeleteOpen(true)}
        >
          Delete
        </Button>
      </Box>

      <Paper sx={{ p: 4 }}>
        <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
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
            <Box sx={{ display: 'flex', gap: 2 }}>
              <Button
                type="submit"
                variant="contained"
                disabled={!isDirty || updateMutation.isPending}
              >
                {updateMutation.isPending ? 'Saving...' : 'Save Changes'}
              </Button>
            </Box>
          </Box>
        </Box>

        <Box sx={{ mt: 3, pt: 2, borderTop: 1, borderColor: 'divider' }}>
          <Typography variant="body2" color="text.secondary">
            Created: {new Date(alertDef.createdAt).toLocaleString()}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Updated: {new Date(alertDef.updatedAt).toLocaleString()}
          </Typography>
        </Box>
      </Paper>

      <ConfirmDialog
        open={deleteOpen}
        title="Delete Alert Definition"
        message={`Are you sure you want to delete "${alertDef.name}"? This action cannot be undone.`}
        onConfirm={handleDelete}
        onCancel={() => setDeleteOpen(false)}
        loading={deleteMutation.isPending}
      />
    </Box>
  );
}
