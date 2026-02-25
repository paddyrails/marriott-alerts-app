import { zodResolver } from '@hookform/resolvers/zod';
import { Box, Button, Link as MuiLink, Paper, TextField, Typography } from '@mui/material';
import axios from 'axios';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { z } from 'zod';
import { useAuth } from '../contexts/AuthContext';

const schema = z.object({
  email: z.string().min(1, 'Email is required').email('Invalid email address'),
  password: z.string().min(8, 'Password must be at least 8 characters'),
  name: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

export default function RegisterPage() {
  const { register: authRegister } = useAuth();
  const navigate = useNavigate();
  const [submitting, setSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>({ resolver: zodResolver(schema) });

  const onSubmit = async (data: FormData) => {
    setSubmitting(true);
    try {
      await authRegister(data.email, data.password, data.name);
      toast.success('Account created successfully');
      navigate('/dashboard', { replace: true });
    } catch (err) {
      const message =
        axios.isAxiosError(err) && err.response?.data?.error?.message
          ? err.response.data.error.message
          : 'Registration failed';
      toast.error(message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '70vh' }}>
      <Paper sx={{ p: 4, maxWidth: 420, width: '100%' }} elevation={3}>
        <Typography variant="h5" fontWeight={600} mb={3} textAlign="center">
          Create Account
        </Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
          <TextField
            label="Name (optional)"
            fullWidth
            margin="normal"
            autoComplete="name"
            {...register('name')}
          />
          <TextField
            label="Email"
            fullWidth
            margin="normal"
            autoComplete="email"
            error={!!errors.email}
            helperText={errors.email?.message}
            {...register('email')}
          />
          <TextField
            label="Password"
            type="password"
            fullWidth
            margin="normal"
            autoComplete="new-password"
            error={!!errors.password}
            helperText={errors.password?.message}
            {...register('password')}
          />
          <Button
            type="submit"
            variant="contained"
            fullWidth
            size="large"
            disabled={submitting}
            sx={{ mt: 2, mb: 2 }}
          >
            {submitting ? 'Creating account...' : 'Register'}
          </Button>
          <Typography variant="body2" textAlign="center">
            Already have an account?{' '}
            <MuiLink component={Link} to="/login">
              Sign In
            </MuiLink>
          </Typography>
        </Box>
      </Paper>
    </Box>
  );
}
