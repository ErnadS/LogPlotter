phase = load('phase.txt');
figure;
plot(phase, 'LineWidth', 1.2);
hold on;
grid on;
axis([0 length(phase) -0.000108 4.5e-5]);
xlabel('Time [s]');
ylabel('Phase difference [s]');