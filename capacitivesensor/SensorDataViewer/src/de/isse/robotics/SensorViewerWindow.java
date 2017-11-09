package de.isse.robotics;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.GridLayout;
import java.awt.image.BufferedImage;
import java.util.stream.DoubleStream;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.SwingConstants;
import javax.swing.SwingUtilities;
import javax.swing.Timer;

public class SensorViewerWindow extends JFrame {
	private static final long serialVersionUID = 1922389429797198458L;
	private JLabel[] lblRaw;
	private JLabel[] lblValue;
	private JLabel[] lblSD;
	private double[] basis;
	private double scale = 0.1;
	private double[] value;
	private double[] mean;
	private final int HISTSIZE = 14;
	private int histpos = 0;
	private double[][] history = new double[HISTSIZE][];
	
	public SensorViewerWindow(SensorModel model) {
		super("ISSE Capacitive Sensor Data Viewer");
		setDefaultCloseOperation(EXIT_ON_CLOSE);
		setSize(1000, 400);
		setLocationRelativeTo(null);
		createLayout(4);

		model.addListener(values -> {
			SwingUtilities.invokeLater(() -> {
				history[histpos] = values;
				if (values.length != lblValue.length)
					createLayout(values.length);
				histpos = (histpos + 1) % HISTSIZE;
				for (int i = 0; i < values.length; i++) {
					value = values;
					lblRaw[i].setText(String.format("%10.6f", value[i]));
					lblValue[i].setText(String.format("%10.6f", value[i] - basis[i]));

					double sum = 0;
					for (int h = 0; h < HISTSIZE; h++) {
						if (history[h] == null)
							return;
						sum += history[h][i];
					}
					mean[i] = sum / HISTSIZE;
					double error = 0;
					for (int h = 0; h < HISTSIZE; h++) {
						error += Math.pow(mean[i] - history[h][i], 2);
					}
					double sd = Math.sqrt(error / HISTSIZE);
					lblSD[i].setText(String.format("%10.6f", sd));
					repaint();
				}
			});
		});
		new Timer(2000, ae -> {
			basis = DoubleStream.of(mean).toArray();
			if (mean.length > 0 && Double.isFinite(mean[0]))
				((Timer) ae.getSource()).stop();
		}).start();
	}
	
	public double[] getMean() {
		return mean;
	}

	private void createLayout(int rows) {
		getContentPane().removeAll();
		JPanel pnlMain = new JPanel(new GridLayout(rows + 1, 2, 5, 10));
		pnlMain.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 5));
		getContentPane().add(pnlMain, BorderLayout.CENTER);

		lblRaw = new JLabel[rows];
		lblValue = new JLabel[rows];
		lblSD = new JLabel[rows];
		resetScale();

		Font textFont = new Font(Font.MONOSPACED, Font.BOLD, 16);
		Font labelFont = new Font(Font.SANS_SERIF, Font.PLAIN, 16);

		for (int i = 0; i < rows; i++) {
			JPanel pnlValues = new JPanel(new GridLayout(3, 2, 5, 0));
			pnlMain.add(pnlValues);

			JLabel lblCap = new JLabel("Capacitance: ");
			lblCap.setHorizontalAlignment(SwingConstants.RIGHT);
			lblCap.setFont(labelFont);
			pnlValues.add(lblCap);
			lblRaw[i] = new JLabel();
			lblRaw[i].setFont(textFont);
			pnlValues.add(lblRaw[i]);

			JLabel lblDifference = new JLabel("Relative: ");
			lblDifference.setHorizontalAlignment(SwingConstants.RIGHT);
			lblDifference.setFont(labelFont);
			pnlValues.add(lblDifference);
			lblValue[i] = new JLabel();
			lblValue[i].setFont(textFont);
			pnlValues.add(lblValue[i]);

			JLabel lblError = new JLabel("Noise: ");
			lblError.setHorizontalAlignment(SwingConstants.RIGHT);
			lblError.setFont(labelFont);
			pnlValues.add(lblError);
			lblSD[i] = new JLabel();
			lblSD[i].setFont(textFont);
			pnlValues.add(lblSD[i]);

			final int j = i;
			JPanel pnlDisp = new JPanel() {
				private static final long serialVersionUID = -7221369823279706019L;
				private Color dark = new Color(0.0f, 0.0f, 0.0f, 0.2f);
				private Color light = new Color(1.0f, 1.0f, 1.0f, 0.15f);
				private BufferedImage buffer;
				private Graphics bg;

				protected void paintComponent(java.awt.Graphics g) {
					if (buffer == null || buffer.getHeight() != getHeight() || buffer.getWidth() != getWidth()) {
						buffer = new BufferedImage(getWidth(), getHeight(), BufferedImage.TYPE_3BYTE_BGR);
						bg = buffer.getGraphics();
						bg.setColor(Color.WHITE);
						bg.fillRect(0, 0, getWidth(), getHeight());
					}
					bg.setColor(light);
					bg.fillRect(0, 0, getWidth(), getHeight());
					bg.setColor(dark);
					int valueWidth = (int) (getWidth() * (0.25 + (value[j] - basis[j]) / scale));
					int meanWidth = (int) (getWidth() * (0.25 + (mean[j] - basis[j]) / scale));
					bg.fillRect(0, 6, valueWidth, 6);
					bg.fillRect(0, 12, meanWidth, getHeight() - 24);
					bg.fillRect(0, 12, meanWidth, getHeight() - 24);
					bg.fillRect(0, 12, meanWidth, getHeight() - 24);
					bg.fillRect(0, getHeight() - 12, valueWidth, 6);
					g.drawImage(buffer, 0, 0, null);
				};
			};
			pnlMain.add(pnlDisp);
		}

		JPanel pnlResetCalib = new JPanel(new GridLayout(2, 1, 5, 5));
		pnlMain.add(pnlResetCalib);
		JButton zero = new JButton("Reset");
		zero.addActionListener(e -> {
			resetScale();
		});
		pnlResetCalib.add(zero);

		JButton reset = new JButton("Calibrate");
		reset.addActionListener(e -> {
			basis = DoubleStream.of(mean).toArray();
		});
		pnlResetCalib.add(reset);

		JPanel pnlScale = new JPanel(new GridLayout(2, 1, 5, 5));
		pnlMain.add(pnlScale);
		JButton decscale = new JButton("Scale +");
		decscale.addActionListener(e -> {
			scale /= 1.3;
		});
		pnlScale.add(decscale);

		JButton incscale = new JButton("Scale -");
		incscale.addActionListener(e -> {
			scale *= 1.3;
		});
		pnlScale.add(incscale);

	}

	private void resetScale() {
		basis = new double[lblValue.length];
		mean = new double[lblValue.length];
		history = new double[HISTSIZE][];
		histpos = 0;
		scale = 0.15;
	}
	
}
